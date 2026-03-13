export interface RegisterUserRequest {
    email: string
    firstName: string
    middleName?: string;
    lastName: string;
    image?: string;
    password: string;
    confirmPassword: string;
}