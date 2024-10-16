import fetchWeb from './axiosUtils';

const enum Api {
    NO_Authorization_Page = 'http://localhost:51169/api/NoAuth/Index',
    LOGIN_PAGE = 'http://localhost:51169/api/Authed/login',
    Authorization_Page = "http://localhost:51169/api/Authed",
}

export const viewNoAuthodPage = ()=>fetchWeb('get',Api.NO_Authorization_Page,null);

export const login = (data:any)=>fetchWeb('post',Api.LOGIN_PAGE,data);

export const viewAuthodPage = ()=>fetchWeb('get',Api.Authorization_Page,null);