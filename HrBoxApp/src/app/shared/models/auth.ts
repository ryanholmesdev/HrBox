import { GenericResponse } from './generic';

export class LoginRequest {
	Email: string;
	Password: string;
}

export class LoggedInUser {
	firstName: string;
	lastName: String;
	email: string;
	tokens: TokenModel;

	constructor(firstname: string, lastname: string, email: string, tokens: TokenModel) {
		this.firstName = firstname;
		this.lastName = lastname;
		this.email = email;
		this.tokens = tokens;
	}
}

export class TokenModel {
	token: string;
	refreshToken: string;
}

export class LoginResponse {
	response: GenericResponse;
	tokens: TokenModel;
}

export class TokenProps {
	emailVerified: boolean;
	expiryDate: number;
	iat: number;
	nfp: number;
	userId: number;
	constructor(emailVerified: boolean, expiryDate: number, iat: number, nfp: number, userId: number) {
		this.emailVerified = emailVerified;
		this.expiryDate = expiryDate;
		this.iat = iat;
		this.nfp = nfp;
		this.userId = userId;
	}
}
