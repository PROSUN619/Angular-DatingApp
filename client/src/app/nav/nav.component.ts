import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  loggedIn : boolean;
  //currentUser$ : Observable<User>;

  constructor(public accountService : AccountService, private router : Router, private toastr : ToastrService) { }


  ngOnInit(): void {
   //this.currentUser$ = this.accountService.currentUser$;
  }

  login(){
    //console.log(this.model);    
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.router.navigateByUrl('/members');
    })
  }

  
  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe(user => {
  //     this.loggedIn = !!user;
  //     // !!means if user is not null then it will convert it to boolean value
  //   }, error => {
  //     console.log(error);      
  //   });
  // }

}
