import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, RegisterRequest } from '../shared/models/user';
import { GenericResponse } from '../shared/models/generic';

@Injectable({
	providedIn: 'root'
})
export class UserService {
	private baseUrl: string = environment.apiUrl + 'api/User/';

	constructor(private httpClient: HttpClient) {}

	registerUser(request: RegisterRequest): Observable<GenericResponse> {
		return this.httpClient.post<GenericResponse>(this.baseUrl + 'CreateUser', request);
	}

	getAllUsers(): Observable<User[]> {
		return this.httpClient.get<User[]>(this.baseUrl + 'GetUsers');
	}
}
