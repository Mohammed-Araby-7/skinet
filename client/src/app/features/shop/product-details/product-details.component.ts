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
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatAnchor,
     MatIcon, MatFormField, MatLabel, MatInput, MatDivider
      ,
    FormsModule
    ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit{

  private ShopService= inject(ShopService);
  private ActivatedRoute = inject(ActivatedRoute);
  private CartService = inject(CartService)
  product?:Product;
  quantityInCart=0;
  quantity=1;
  ngOnInit(): void {
      this.loadProduct();
  }
  

  loadProduct()
  {
    const id = this.ActivatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.ShopService.getProduct(+id).subscribe({
      next:product => {
        this.product = product,
        this.updateQuantityInCart();
      },
      error:err => console.log(err)
    })
  }
  updateCart(){
    if(!this.product) return;
    if(this.quantity > this.quantityInCart){
      const itemsToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart +=itemsToAdd;
      this.CartService.addItemToCart(this.product,itemsToAdd);
    }else{
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -=itemsToRemove;
      this.CartService.removeItemFromCart(this.product.id,itemsToRemove);
    }
  }


  updateQuantityInCart(){
    this.quantityInCart = this.CartService.cart()?.items.
    find(x=> x.productId === this.product?.id)?.quantity || 0;
  }
  getButtonText(){
    return this.quantityInCart > 0 ? 'Update Cart ' : 'Add to Cart';
  }
} 
