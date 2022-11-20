import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseServiceService } from '../base-service.service';
import { City } from '../_models/city';

@Injectable({
  providedIn: 'root'
})
export class CityServiceService extends BaseServiceService {

  constructor(httpClient: HttpClient, @Inject("BASE_URL") baseUrl: string) {
    super(httpClient, baseUrl);
  }

  getData<ApiResult>(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string, filterOrder: string): Observable<ApiResult> {
    var url: string = this.baseUrl + 'api/Cities';
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterOrder != null) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterOrder", filterOrder);
    }


    return this.httpClient.get<ApiResult>(url);
  }
  get<City>(id: number): Observable<City> {
    return this.httpClient.get<City>(this.baseUrl + 'api/Cities' + id);
  }
  put<City>(item: City): Observable<City> {
    return this.httpClient.put<City>(this.baseUrl + 'api/Cities', item);
  }
  post<City>(item: City): Observable<City> {
    var url = this.baseUrl + 'api/Cities';
    return this.httpClient.post<City>(url, item);
  }

}
