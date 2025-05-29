export interface User{
    id: string,
    email: string,
    name: string,
    surname: string,
    birthDate : Date,
    refreshToken : string,
    refreshExpires : Date
}