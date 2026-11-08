import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../models/order';


@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  orderComplete = false;

  CreateOrder(OrderToCreate:OrderToCreate)
  {
    return this.http.post<Order>(this.baseUrl + 'orders' , OrderToCreate);
  }

  getOrdersForUser(){
    return this.http.get<Order[]>(this.baseUrl + 'orders');
  }

  getOrderDetailed(id:number){
    return this.http.get<Order>(this.baseUrl + 'orders/' + id);
  }

  
}
