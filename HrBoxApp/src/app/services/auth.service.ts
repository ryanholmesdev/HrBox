import { Injectable } from '@angular/core';
import { BehaviorSubject, throwError, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { LoggedInUser, LoginRequest, LoginResponse } from '../shared/models/auth';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root'
})
export class AuthService {
	private baseUrl: string = environment.apiUrl + '/api/User/';

	$loggedInUser: BehaviorSubject<LoggedInUser> = null;

	constructor(private httpClient: HttpClient) {}

	login(request: LoginRequest): Observable<LoginResponse> {
		return this.httpClient.post<LoginResponse>(this.baseUrl + 'login', request).pipe(
			map(response => {
				if (response.Response.Success) {
					const user = new LoggedInUser(response.Token);
					this.$loggedInUser.next(user);
					return response;
				} else {
					return response;
				}
			}),
			catchError(err => {
				return throwError(err);
			})
		);
	}
}
