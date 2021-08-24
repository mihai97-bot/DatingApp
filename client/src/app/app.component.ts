import { Component } from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'The Dating app page';
  users : any;

  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.getUsers();
  }
    getUsers() {
    this.http.get("https://localhost:5001/users").subscribe(response => {
      this.users = response;
    }, (error) => {
    console.log(error);
      
    })
     }
}