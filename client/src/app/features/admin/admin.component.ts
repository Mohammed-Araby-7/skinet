import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule} from '@angular/material/table'
import { AdminService } from '../../core/services/admin.service';
import { OrderParams } from '../../models/orderParams';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Order } from '../../models/order';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltip, MatTooltipModule } from '@angular/material/tooltip';
import { MatLabel, MatSelectChange, MatSelectModule } from '@angular/material/select';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../core/services/dialog.service';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-admin',
  standalone:true,
  imports: [
 MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatTooltipModule,
    MatSelectModule,
    MatFormFieldModule,
    DatePipe,
    CurrencyPipe,
    RouterLink
],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements AfterViewInit , OnInit{
  // upstash of Redis
  // master-urchin-110007.upstash.io:6379, password=gQAAAAAAAa23AAIgcDJhYTJmZDcxOTkxZmM0NDJiYWQ3MDI2YzZlMWYwZmNhZA, ssl=true , abortConnection=False
   private adminService = inject(AdminService);
  private dialogService = inject(DialogService);

  // Table configuration
  displayColumns: string[] = [
    'id',
    'buyerEmail',
    'orderDate',
    'total',
    'status',
    'actions'
  ];

  dataSource = new MatTableDataSource<Order>([]);

  // Pagination
  orderParams = new OrderParams();
  totalItems = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  // Status filter options
  statusOptions = [
    'All',
    'PaymentReceived',
    'PaymentMismatch',
    'Refunded',
    'Pending'
  ];

  ngOnInit(): void {
    this.loadOrders();
  }

  ngAfterViewInit(): void {
    // Server-side pagination is handled by the API.
    // Do not assign paginator to dataSource.
  }

  // Load orders from API
  loadOrders(): void {

    this.adminService.getOrders(this.orderParams).subscribe({

      next: response => {

        this.dataSource.data = response.data;
        this.totalItems = response.count;

      },

      error: error => {
        console.error('Error loading orders:', error);
      }

    });

  }

  // Handle pagination
  onPageChange(event: PageEvent): void {

    this.orderParams.pageNumber = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;

    this.loadOrders();

  }

  // Handle status filter
  onFilterSelect(event: MatSelectChange): void {

    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;

    if (this.paginator) {
      this.paginator.firstPage();
    }

    this.loadOrders();

  }

  // Open refund confirmation dialog
  async OpenConfirmDialog(id: number): Promise<void> {

    const confirmed = await this.dialogService.confirm(
      'Confirm refund',
      'Are you sure you want to issue this refund? This cannot be undone.'
    );

    if (confirmed) {
      this.refundOrder(id);
    }

  }

  // Refund order
  refundOrder(id: number): void {

    this.adminService.refundOrder(id).subscribe({

      next: order => {

        this.dataSource.data = this.dataSource.data.map(o =>
          o.id === id ? order : o
        );

      },

      error: error => {
        console.error('Error refunding order:', error);
      }

    });

  }
}
