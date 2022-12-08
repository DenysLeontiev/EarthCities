import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { from } from 'rxjs';
import "rxjs/add/operator/map"
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DataChartService {

  constructor(private httpClient: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }

  url = this.baseUrl + 'api/Countries';
  displayDataChart() {
    return this.httpClient.get(this.url)
      .pipe(map((result) => result));
  }

}
