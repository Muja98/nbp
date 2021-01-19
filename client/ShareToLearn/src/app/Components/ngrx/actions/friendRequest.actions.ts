import {Action} from '@ngrx/store';

export const ADD_FRINEDREQUEST          = '[NUMBER] Add'
export const REMOVE_FRIENDREQUEST       = '[NUMBER] Remove'

export class AddFriendRequest implements Action{
    readonly type = ADD_FRINEDREQUEST
    constructor(public payload: number){}
}

export class RemoveFrinedRequesr implements Action{
    readonly type = REMOVE_FRIENDREQUEST
    constructor(public payload: number){}
}


export type Actions = AddFriendRequest | RemoveFrinedRequesr