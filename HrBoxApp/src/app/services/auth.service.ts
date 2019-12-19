import { Injectable } from '@angular/core';
import { BehaviorSubject, throwError, Observable } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { LoggedInUser, LoginRequest, LoginResponse, TokenModel, TokenProps } from '../shared/models/auth';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root'
})
export class AuthService {
	private baseUrl: string = `${environment.apiUrl}/api/auth/`;
	private tokenExpiryTimeout: any = null;
	private isRefreshing: boolean = false;

	$loggedInUser: BehaviorSubject<LoggedInUser> = new BehaviorSubject(null);

	constructor(private httpClient: HttpClient) {
		this.checkTimeoutOfToken();
	}

	login(request: LoginRequest): Observable<LoginResponse> {
		return this.httpClient.post<LoginResponse>(this.baseUrl + 'login', request).pipe(
			map(response => {
				if (response.response.success) {
					const user = new LoggedInUser('', '', '', response.tokens);
					this.$loggedInUser.next(user);
					this.saveTokens(response.tokens);
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

	createRefreshTokenObservable(tokens: TokenModel) {
		return this.httpClient.post<TokenModel>(`${this.baseUrl}RefreshToken`, tokens).pipe(
			tap((tokens: TokenModel) => {
				this.saveTokens(tokens);
			})
		);
	}

	refreshToken(tokens: TokenModel) {
		if (!this.isRefreshing) {
			this.isRefreshing = true;
			// todo retry the refresh.
			console.log('Refresh of token is needed');
			this.createRefreshTokenObservable(tokens).subscribe(
				(data: TokenModel) => {
					if (data) {
						this.saveTokens(data);
					}
				},
				error => {
					console.log(error);
					console.log('Unable to refresh');
					// remove tokens.
					this.removeTokens();
				}
			);
		}
	}

	logout(): void {
		this.httpClient.get<boolean>(`${this.baseUrl}logout`).subscribe(
			(response: boolean) => {
				return response;
			},
			error => {
				return false;
			}
		);
	}

	private saveTokens(tokens: TokenModel) {
		localStorage.setItem('token', tokens.token);
		localStorage.setItem('refreshToken', tokens.refreshToken);
	}

	private decodeToken(rawToken: string): TokenProps {
		let base64Url = rawToken.split('.')[1];
		let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
		const jsonPayload = decodeURIComponent(
			atob(base64)
				.split('')
				.map(function(c) {
					return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
				})
				.join('')
		);

		const token = JSON.parse(jsonPayload);
		const isUserEmailVerified = token.EmailVerified.toLowerCase() === 'true';
		const tokenProps = new TokenProps(isUserEmailVerified, token.exp, token.iat, token.nbf, parseInt(token.unique_name));
		return tokenProps;
	}

	private getTokens(): TokenModel {
		if (this.$loggedInUser.getValue()) {
			return this.$loggedInUser.getValue().tokens;
		}
		const token: string = localStorage.getItem('token');
		const refreshToken: string = localStorage.getItem('refreshToken');
		if (token && refreshToken) {
			let tokenModel = new TokenModel();
			tokenModel.token = token;
			tokenModel.refreshToken = refreshToken;
			return tokenModel;
		} else {
			return null;
		}
	}

	private isTokenExpired(tokenProp: TokenProps) {
		const now = new Date();
		const isExpired = now.getTime() > tokenProp.expiryDate * 1000;
		return isExpired;
	}

	private checkTimeoutOfToken(): void {
		const tokens = this.getTokens();
		if (tokens) {
			const tokenProp: TokenProps = this.decodeToken(tokens.token);
			const isTokenExpired: boolean = this.isTokenExpired(tokenProp);
			if (isTokenExpired) {
				this.refreshToken(tokens);
			} else {
				this.setExpiryTimeout(tokenProp);
			}
		}
	}

	private setExpiryTimeout(tokenProp: TokenProps): void {
		const interval = new Date(tokenProp.expiryDate * 1000).getTime() - new Date().getTime();
		this.tokenExpiryTimeout = setTimeout(() => {
			this.refreshToken(this.getTokens());
		}, interval);
	}

	private removeTokens() {
		localStorage.removeItem('token');
		localStorage.removeItem('refreshToken');
	}
}
