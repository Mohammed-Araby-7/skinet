import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { OrderParams } from '../../models/orderParams';
import { Pagination } from '../../models/pagination';
import { Order } from '../../models/order';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  // Get paginated orders with optional status filter
  getOrders(orderParams: OrderParams) {

    let params = new HttpParams();

    if (orderParams.filter && orderParams.filter !== 'All') {
      params = params.append('status', orderParams.filter);
    }

    params = params.append('pageIndex', orderParams.pageNumber);
    params = params.append('pageSize', orderParams.pageSize);

    return this.http.get<Pagination<Order>>(
      this.baseUrl + 'admin/orders',
      { params }
    );
  }

  // Get a specific order
  getOrder(id: number) {
    return this.http.get<Order>(
      this.baseUrl + 'admin/orders/' + id
    );
  }

  // Refund an order
  refundOrder(id: number) {
    return this.http.post<Order>(
      this.baseUrl + 'admin/orders/refund/' + id,
      {}
    );
  }
}
