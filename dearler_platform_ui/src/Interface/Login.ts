//如果在这里显式导出，就需要在使用时添加 import { IloginInfo } from '@/Interface/Login'; 来显式引入
export interface IloginInfo {
    userNo: string,
    password: string,
    login: Function
}