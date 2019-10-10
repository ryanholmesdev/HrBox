import { Component, OnInit } from '@angular/core';
import { UserService } from './services/user.service';
import { User } from './shared/models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'HrBoxApp';

  constructor(private userService: UserService){}
  
  ngOnInit(): void{

    this.userService.getAllUsers()
    .subscribe(
      (users: any) => {
        users.forEach(user => {
          console.log(user);
        });
      },
      error => {
        console.log(error);
      });
  }
}
