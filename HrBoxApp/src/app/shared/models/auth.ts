import { GenericResponse } from './generic';

export class LoginRequest {
	Email: string;
	Password: string;
}

export class LoggedInUser {
	FirstName: string;
	LastName: String;
	Email: string;
	Tokens: TokenModel;

	constructor(tokens: TokenModel) {
		this.Tokens = tokens;
	}
}

export class TokenModel {
	Token: string;
	RefreshToken: string;
}

export class LoginResponse {
	Response: GenericResponse;
	Token: TokenModel;
}
