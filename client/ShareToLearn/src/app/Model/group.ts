export class Group
{
    id:number = 0;
    group:any = {
        field: "",
        name: "",
        description: ""
    }
    student:any = {
        id: 0,
        student: {
            firstName: "",
            lastName: "",
            dateOfBirth: "",
            email: "",
        }
    }
    constructor(){}
}