export interface IkunksPagedResponse {
  items: IkunkResponse[];
}

export interface IkunkResponse {
  id: string;
  name: string;
  fullName: string;
  description?: string;
  orderPosition: number;
  activeFrom: string;
  activeTo: string;
  warehouseId: string;
}