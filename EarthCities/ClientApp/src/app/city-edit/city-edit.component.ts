import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { City } from '../_models/city';
import { error } from 'protractor';
import { Country } from '../_models/country';
import { from, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseformComponent } from '../baseform/baseform.component';
import { CityServiceService } from '../cities/city-service.service';
@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.css']
})
export class CityEditComponent extends BaseformComponent implements OnInit {

  constructor(private cityService: CityServiceService, private httpClient: HttpClient, private router: Router, private activatedRoute: ActivatedRoute, @Inject("BASE_URL") private baseUrl: string) {
    super();
  }

  title: string;
  form: FormGroup;
  city: City;
  id?: number;
  countries: Country[];

  ngOnInit(): void {
    this.form = new FormGroup({
      "name": new FormControl('', Validators.required),
      "latitude": new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      "longitude": new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      "countryId": new FormControl('', Validators.required),
    }, null, this.isDupeCity())

    this.loadData();
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.get("name").value;
      city.latitude = +this.form.get("latitude").value;
      city.longitude = +this.form.get("longitude").value;
      city.countryId = +this.form.get("countryId").value;
      var url = this.baseUrl + "api/Cities/IsDupeCity";
      return this.httpClient.post<boolean>(url, city).pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }));
    }
  }

  loadData() {
    this.loadCountries();

    this.id = +this.activatedRoute.snapshot.paramMap.get('id');
    if (this.id) {
      var url = this.baseUrl + 'api/Cities/' + this.id;

      this.httpClient.get<City>(url).subscribe((response) => {
        this.city = response;
        this.title = "Editing " + this.city.name;
        this.form.patchValue(this.city);
      }, error => {
        console.log(error);
      });
    }
    else {
      this.title = "Create new city";
    }
  }

  loadCountries() {
    var url = this.baseUrl + 'api/Countries';
    var params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name")
    this.httpClient.get<any>(url, { params }).subscribe((response) => {
      this.countries = response.data;
    }, error => {
        console.log(error);
    });
  }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};

    city.name = this.form.get('name').value;
    city.latitude = +this.form.get('latitude').value;
    city.longitude = +this.form.get('longitude').value;
    city.countryId = +this.form.get('countryId').value;

    if (this.id) {
      this.cityService.put<City>(city).subscribe((response) => {
        console.log("City is added " + city.name + " | " + city.id);
        this.router.navigate(['/cities']);
      }, error => {
        console.log(error);
      })
    } else {
      this.cityService.post<City>(city).subscribe(response => {
        console.log("City was added sucessfully with id: " + response.id);
        this.router.navigate(['/cities']);
      }, error => {
          console.log(error);
      })
    }
  }

  getControl(name: string) {
    return this.form.get(name);
  }

  isValid(name: string) { // true if valid
    var e = this.getControl(name);
    return e && e.invalid
  }

  isChanged(name: string) { // true if changed
    var e = this.getControl(name);
    return e && (e.touched || e.dirty);
  }

  hasError(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched) && e.invalid;
  }
}
