import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/members';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userparams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  user: User | undefined;
  genderList = [{value:'male', display: 'Males'},{value:'female', display: 'Females'}]

  constructor(private memberservice: MembersService, private accountService: AccountService) {
    this.initUser();
  }

  initUser(){
    this.userParams = this.memberservice.getUserParams();
    this.loadMembers();
  }
  
  resetFilters(){
      this.userParams = this.memberservice.resetUserParams();
      this.loadMembers();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams){
      this.memberservice.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
            //console.log(this.pagination);
          }
        }
      });
    }
    //debugger;    
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

}
