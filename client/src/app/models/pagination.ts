import { IProduct } from "./product";

export class IPagination {
    pageIndex: number;
    pageSize: number;
    count: number;
    data: IProduct[];
}