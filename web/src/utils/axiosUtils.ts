
import axios from 'axios';



const fetchWeb = (method:string,url:string,data:any)=>axios({
    method,
    url,
    data
});

export const accessTokenKey = "access-token";
export const refreshAccessTokenKey = `x-${accessTokenKey}`;

axios.interceptors.request.use((config:any)=>{
    const token = localStorage.getItem(accessTokenKey);
    if(token){
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
},(error:any)=>{
    return Promise.reject(error);
});

export default fetchWeb;