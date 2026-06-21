import { Component, inject, OnInit } from '@angular/core';
import {MatCard} from '@angular/material/card';
import {MatDialog} from '@angular/material/dialog';
import { Product } from '../../models/product';
import { ShopService } from '../../core/services/shop.service';
import { ProductItemComponent } from "./product-item/product-item.component";
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { MatMenu, MatMenuTrigger } from "@angular/material/menu";
import { MatPaginator, PageEvent} from "@angular/material/paginator";
import { MatListModule, MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { ShopParams } from '../../models/ShopParams';
import { Pagination } from '../../models/pagination';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop',
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatListModule,
    MatPaginator,
    FormsModule,
    MatIconButton
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})  
export class ShopComponent implements OnInit {
   baseUrl = 'https://localhost:5001/api/'
  private dialogService = inject(MatDialog)

  private ShopService = inject(ShopService);

  shopParams = new ShopParams()

  products?:Pagination<Product>;
  
  sortOptions = [
    {name:'Alphabetical' , value:'name'},
    {name:'Price: Low-High' , value:'priceAsc'},
    {name:'Price: High-low',value:'priceDesc'},
  ]


  ngOnInit(): void {
      this.intializeShop();
  }

  intializeShop()
  {
    this.ShopService.getBrands();
    this.ShopService.getTypes();
    this.getProducts()
  }

    getProducts()
    {
      this.ShopService.getProducts(this.shopParams).subscribe({
        next:res=>this.products = res,
        error:err=> console.log(err)
      })
    }
    handlePageEvent(event:PageEvent)
    {
      this.shopParams.pageNumber = event.pageIndex + 1;
      this.shopParams.pageSize = event.pageSize;
      this.getProducts();
    }

    onSearchChange()
    {
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }

  openFilterDialog()
  {
    const dialogRef = this.dialogService.open(FiltersDialogComponent ,
      {
        minWidth:'500px',
        data:{
          selectedBrands:this.shopParams.brands,
          selectedTypes:this.shopParams.types
        }
      }
    );
    dialogRef.afterClosed().subscribe({
      next:result =>{
        if(result)
        {
          console.log(result);
          this.shopParams.brands = result.selectedBrands;
          this.shopParams.types = result.selectedTypes;
          this.shopParams.pageNumber = 1;
          this.getProducts();
        }
      }
    })
  }

  onSortChange(event:MatSelectionListChange){
    const selectedOption = event.options[0];
    if(selectedOption)
    {
      this.shopParams.sort=selectedOption.value;
      this.shopParams.pageNumber =1;
      this.getProducts();
    }

  }
}
