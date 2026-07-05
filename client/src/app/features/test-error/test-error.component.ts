import { HttpClient } from '@angular/common/http'
import { Component, inject } from '@angular/core'
import {MatButtonModule} from '@angular/material/button'

@Component({
  selector: 'app-test-error',
  imports: [
   MatButtonModule
  ],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss',
})
export class TestErrorComponent {
  baseUrl = 'https://localhost:5001/api/'
  private http = inject(HttpClient);
  validationErros?: string[];

  get404Error(){
    this.http.get(this.baseUrl + 'buggy/notfound').subscribe({
      next: res => console.log(res),
      error: err => console.log(err)
    })
  }

  get400Error(){
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe({
      next: res => console.log(res),
      error: err => console.log(err)
    })
  }

  get401Error(){
    this.http.get(this.baseUrl + 'buggy/unauthorized').subscribe({
      next: res => console.log(res),
      error: err => console.log(err)
    })
  }

  get500Error(){
    this.http.get(this.baseUrl + 'buggy/internalerror').subscribe({
      next: res => console.log(res),
      error: err => console.log(err)
    })
  }

  get400ValidationError(){
    this.http.post(this.baseUrl + 'buggy/validationerror' , {}).subscribe({
      next: res => console.log(res),
      error: err => this.validationErros = err
    })
  }

 
}
