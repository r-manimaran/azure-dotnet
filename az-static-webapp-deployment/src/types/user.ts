export interface UserResponse {
    results: {
        email: string;
        phone: string;
        name: {
            first: string;
            last: string;
        };    
    }[];
}