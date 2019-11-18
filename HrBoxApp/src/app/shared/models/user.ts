export interface User {
	FirstName: string;
	LastName: string;
	Password: string;
	FullName: string;
	DateOfBirth: Date;
	DateCreated: Date;
}
export class RegisterRequest {
	FirstName: string;
	LastName: string;
	Email: string;
	Password: string;
}
