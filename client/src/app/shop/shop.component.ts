import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/productBrand';
import { IProductType } from '../shared/models/productType';
import { ShopParams } from '../shared/models/shop-params';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search', {static: true}) searchTerm: ElementRef; 
  products: IProduct[];
  brands: IProductBrand[];
  types: IProductType[];
  shopParams = new ShopParams()
  totalCounts: number;
  sortOptions = 
  [
    {name: 'Alphabetical', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'},
  ]

  constructor(private shopService: ShopService) { }

  ngOnInit(): void {
    this.getProductBrands();
    this.getProductTypes();
    this.getProducts();
  }

  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe(response => {
      this.products = response.data;
      this.shopParams.pageNumber = response.pageIndex;
      this.shopParams.pageSize = response.pageSize;
      this.totalCounts = response.count;
    });
  }

  getProductBrands() {
    this.shopService.getBrands().subscribe(response => {
      this.brands = [{id:0, name: 'All'}, ...response];
    });
  }

  getProductTypes() {
    this.shopService.getTypes().subscribe(response => {
      this.types = [{id: 0, name: 'All'}, ...response];
    });
  }

  onBrandSelected(brandId: number) {
    this.shopParams.brandIdSelected = brandId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onTypeSelected(typeId: number)
  {
    this.shopParams.typeIdSelected = typeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onSortSelected(sort: string)
  {
    this.shopParams.sortSelected = sort;
    this.getProducts();
  }

  onPageChanged(event: any) {
    if(this.shopParams.pageNumber !== event)
    {
      this.shopParams.pageNumber = event;
    this.getProducts();
    }
  }

  onSearch() {
    this.shopParams.search = this.searchTerm.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.getProducts();
  }
}
