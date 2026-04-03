export interface RegisterUserRequest {
    username: string;
    email: string;
    firstName: string
    middleName?: string;
    lastName: string;
    image?: string;
    password: string;
    confirmPassword: string;
    enable2Fa: boolean;
}

export interface RegisterUserFormRequest {
    username: string;
    email: string;
    firstName: string
    middleName: string;
    lastName: string;
    image: string;
    password: string;
    confirmPassword: string;
    enable2Fa: boolean;
}

export function toRegisterUserFormRequest(request: RegisterUserRequest) {
    return {
        ...request,
        middleName: request.middleName ?? '',
        image: request.image ?? ''
    };
}

export function toRegisterUserRequest(required: RegisterUserFormRequest) {
    return {
        ...required,
        middlename: required.middleName ? undefined : required.middleName,
        image: required.image ? undefined : required.image
    };
}