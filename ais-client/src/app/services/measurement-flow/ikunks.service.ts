import { httpResource } from '@angular/common/http';
import { Injectable } from '@angular/core';

export interface Ikunk {
  id: string;
  name: string;
  fullName: string;
  description?: string;
  orderPosition: number;
  activeFrom: string;
  activeTo: string;
  warehouseId: string;
}

@Injectable({ providedIn: 'root' })
export class IkunksService {
  readonly ikunks = httpResource<Ikunk[]>(() => ({
    url: 'https://localhost:4001/api/flow/ikunks',
    withCredentials: true,
  }));
}
