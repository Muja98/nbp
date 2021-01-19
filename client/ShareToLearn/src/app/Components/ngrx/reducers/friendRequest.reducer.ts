import * as FrinedRequestActions from './../actions/friendRequest.actions';
import { Action } from '@ngrx/store';

export function reducer (state: number[] = [1], action: FrinedRequestActions.Actions)
{
    switch(action.type)
    {
        case FrinedRequestActions.ADD_FRINEDREQUEST:
                return [...state, action.payload];
        default:
            return state;
    }
}