import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Country } from '../_models/country';
import { BaseformComponent } from '../baseform/baseform.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.css']
})
export class CountryEditComponent extends BaseformComponent implements OnInit, OnDestroy {

  // the view title
  title: string;

  // the form model
  form: FormGroup;

  // the city object to edit or create
  country: Country;

  // the city object id, as fetched from the active route:
  // It's NULL when we're adding a new country,
  // and not NULL when we're editing an existing one.
  id?: number;
  private subscription: Subscription = new Subscription();

  activityLog: string;

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
    super();
    this.loadData();
  }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['',
        Validators.required,
        this.isDupeField("name")
      ],
      iso2: ['',
        [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{2}$/)
        ],
        this.isDupeField("iso2")
      ],
      iso3: ['',
        [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{3}$/)
        ],
        this.isDupeField("iso3")
      ]
    });

    this.subscription.add(this.form.valueChanges.subscribe((response) => {
      if (!this.form.dirty) {
        this.log("Form was loaded");
      }
      else {
        this.log("Form was modified");
      }
    }));

    this.subscription.add(this.form.get("name").valueChanges.subscribe((response) => {
      if (!this.form.dirty) {
        this.log("Form Name was loaded");
      } else {
        this.log("Form Name was modified by a user");
      }
    }));

    this.loadData();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  log(str: string) {
    this.activityLog += "[" + new Date().toLocaleString() + "]" + str + "<br />";
  }

  loadData() {

    // retrieve the ID from the 'id'
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');
    if (this.id) {
      // EDIT MODE

      // fetch the country from the server
      var url = this.baseUrl + "api/Countries/" + this.id;
      this.http.get<Country>(url).subscribe(result => {
        this.country = result;
        this.title = "Edit - " + this.country.name;

        // update the form with the country value
        this.form.patchValue(this.country);
      }, error => console.error(error));
    }
    else {
      // ADD NEW MODE

      this.title = "Create a new Country";
    }
  }

  onSubmit() {

    var country = (this.id) ? this.country : <Country>{};

    country.name = this.form.get("name").value;
    country.iso2 = this.form.get("iso2").value;
    country.iso3 = this.form.get("iso3").value;

    if (this.id) {
      // EDIT mode

      var url = this.baseUrl + "api/Countries/" + this.country.id;
      this.http
        .put<Country>(url, country)
        .subscribe(result => {

          console.log("Country " + country.id + " has been updated.");

          // go back to cities view
          this.router.navigate(['/countries']);
        }, error => console.error(error));
    }
    else {
      // ADD NEW mode
      var url = this.baseUrl + "api/Countries";
      this.http
        .post<Country>(url, country)
        .subscribe(result => {

          console.log("Country " + result.id + " has been created.");

          // go back to cities view
          this.router.navigate(['/countries']);
        }, error => console.error(error));
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {

      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);
      var url = this.baseUrl + "api/Countries/IsDupeField";
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}
