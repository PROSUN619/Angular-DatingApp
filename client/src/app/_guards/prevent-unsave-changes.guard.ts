import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsaveChangesGuard implements CanDeactivate<unknown> {

  canDeactivate(
    component: MemberEditComponent): boolean {
    if (component.editForm.dirty)
      return confirm('Are you sure want to exit? Any unsaved change will be lost');
    return true;
  }

}
