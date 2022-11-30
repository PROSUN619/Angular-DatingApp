import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/members';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userparams';
import { AccountService } from './account.service';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          //debugger
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    });
   }

   getUserParams(){
    return this.userParams;
   }

   setUserParams(params: UserParams){
    this.userParams = params;
   }

   resetUserParams(){
    if (this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
   }

  // getMembers() {
  //   if (this.members.length > 0) return of(this.members);

  //   return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
  //     map((members) => {
  //       this.members = members;
  //       return this.members;
  //     })
  //   );
  // }

  getMembers(userParam: UserParams) {
    let params = this.getPaginationHeaders(userParam.pageNumber, userParam.pageSize);
    const response = this.memberCache.get(Object.values(userParam).join('-'));

    if (response) return of(response);

    params = params.append('minAge', userParam.minAge);
    params = params.append('maxAge', userParam.maxAge);
    params = params.append('gender', userParam.gender);
    params = params.append('orderBy', userParam.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParam).join('-'), response);
        return response;
      })
    )
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );

  }


  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return params;
  }

  //please refer video no 167 if not understand
  getMember(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr,elm) => arr.concat(elm.result), [])
    .find((member:Member) => member.userName === username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member
      })
    );
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

}
