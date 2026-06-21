import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Product } from '../../../models/product';
import { CurrencyPipe } from '@angular/common';
import { MatAnchor } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import {MatFormField, MatLabel } from "@angular/material/form-field"
import {MatInput} from "@angular/material/input"
import { MatDivider } from "@angular/material/divider";

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatAnchor, MatIcon, MatFormField, MatLabel, MatInput, MatDivider],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit{

  private ShopService= inject(ShopService);
  private ActivatedRoute = inject(ActivatedRoute);
  product?:Product;
  ngOnInit(): void {
      this.loadProduct();
  }
  quantity = 1;

  loadProduct()
  {
    const id = this.ActivatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.ShopService.getProduct(+id).subscribe({
      next:product => this.product = product,
      error:err => console.log(err)
    })
  }
} 
