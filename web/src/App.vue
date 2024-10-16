<template>
  <div class="container">
    <div class="box">
      <div class="content">
        <div class="login">
          <button @click="testLogin">login</button>
        </div>
        <div class="go">
          <button @click="test2" style="margin-left: 20px;">access Authod Page</button>
        </div>
        <div class="go">
          <button @click="test" style="margin-left: 20px;">access NoAthod Page</button>
        </div>
      </div>
      <div class="footer">
        {{ msg }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {viewNoAuthodPage,login,viewAuthodPage} from './utils/fetchWeb';
import { ref } from 'vue';
import { accessTokenKey } from './utils/axiosUtils';

const msg = ref('first if you want to access authod page,you will got a error,you must login first');


function testLogin(){
  login({
    email:'user@example.com',
    password:'string'
  }).then((res:any)=>{
    console.log(res);
    msg.value = `success:logined!!`;
    //将token存储到localStorage
    localStorage.setItem(accessTokenKey,res.data.token);
  }).catch((err:any)=>{
    console.log(err);
    msg.value = `error:${err}`;
  });
}

function test() {
  viewNoAuthodPage().then((res: any) => {
      console.log(res);
      msg.value = `success:${res.data}`;
  }).catch((err:any)=>{
    console.log(err);
  });

}

function test2(){
  viewAuthodPage().then((res:any)=>{
    console.log(res);
    msg.value = `success:${res.data}`;
  }).catch((err:any)=>{
    console.log(err);
    msg.value = `error:${err}`;
  });
}

</script>

<style scoped>
.container {
  width: 100vw;
  height: 100vh;
}

.box {
  width: 100%;
  height: 600px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.content {
  display: flex;
  justify-content: center;
  align-items: center;
}

.footer {
  margin-top: 50px;
}
</style>