import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  //baseURL = 'https://localhost:5001/api/';
  baseURL = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  //above code going to create buffer of size 1 which will emit the user data
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) {

  }

  login(model: any) {
    return this.http.post(this.baseURL + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseURL + 'account/register', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    this.presenceService.createHubConnection(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection()
  }

  getDecodedToken(token: string) {
    if (token) {
      //console.log(token.split('.')[1]);
      //console.log(atob(token.split('.')[1]));
      //console.log(JSON.parse(atob(token.split('.')[1])));  
      return JSON.parse(atob(token.split('.')[1])); // atob is used to decoding
    }
  }
}
