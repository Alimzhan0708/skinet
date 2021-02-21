import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IProductBrand } from '../shared/models/productBrand';
import { IProductType } from '../shared/models/productType';
import { map } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shop-params';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http:HttpClient) { }

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();
    if(shopParams.brandIdSelected !== 0)
    {
      params = params.append('brandId', shopParams.brandIdSelected.toString());
    }

    if(shopParams.typeIdSelected !== 0)
    {
      params = params.append('typeId', shopParams.typeIdSelected.toString());
    }

    if(shopParams.search)
    {
      params = params.append('search', shopParams.search);
    }
    
    params = params.append('sort', shopParams.sortSelected);
    params = params.append('pageIndex', shopParams.pageNumber.toString());
    params = params.append('pageSize', shopParams.pageSize.toString());

    return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params}).pipe(map(response => {
      return response.body;
    }));
  }

  getProduct(id: number) {
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  getBrands() {
    return this.http.get<IProductBrand[]>(this.baseUrl + 'products/brands');
  }

  getTypes() {
    return this.http.get<IProductType[]>(this.baseUrl + 'products/types');
  }
}
