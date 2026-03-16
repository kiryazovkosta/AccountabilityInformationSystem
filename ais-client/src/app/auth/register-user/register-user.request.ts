export interface RegisterUserRequest {
    email: string;
    firstName: string
    middleName?: string;
    lastName: string;
    image?: string;
    password: string;
    confirmPassword: string;
}

export interface RegisterUserFormRequest {
    email: string;
    firstName: string
    middleName: string;
    lastName: string;
    image: string;
    password: string;
    confirmPassword: string;
}

export function toRegisterUserFormRequest(request: RegisterUserRequest) {
    return {
        ...request,
        middleName: request.middleName ?? '',
        image: request.image ?? '',
    };
}

export function toRegisterUserRequest(required: RegisterUserFormRequest) {
    return {
        ...required,
        middlename: required.middleName ? undefined : required.middleName,
        image: required.image ? undefined : required.image,
    };
}