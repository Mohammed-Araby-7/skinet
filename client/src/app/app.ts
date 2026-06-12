import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { Product } from './models/product';
import { Pagination } from './models/pagination';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App  implements OnInit{
  baseUrl = 'https://localhost:5001/api/'


  private http = inject(HttpClient);
  products :Product[] = []
  ngOnInit(): void {
      this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({
        next:(res)=>{
          console.log(res);
          this.products = res.data;
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{
          console.log('complete')
        }
      })
  }

  protected title = 'Skient';
}
