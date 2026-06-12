import { HttpClient, httpResource } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CreateWarrantyRecordRequest } from '../models/create-warranty-record.request';
import { WarrantyRecordResponse } from '../models/warranty-record.response';

export interface LinkResponse {
    href: string;
    rel: string;
    method: string;
}

export interface PaginationResponse<T> {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  items: T[];
  links?: LinkResponse[]; 
}

export interface WarrantyRecordListResponse {
  id: string;
  warrantyBrandName: string;
  model: string;
  purchaseDate: Date;
  receiptExists: boolean;
  frontImageExists: boolean;
  backImageExists: boolean;
}

@Injectable({ providedIn: 'root' })
export class WarrantyRecordsService {
  readonly #http = inject(HttpClient);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly warrantyId = signal<string | undefined>(undefined);

  readonly warrantyRecords = httpResource<PaginationResponse<WarrantyRecordListResponse>>(() => ({
    url: `${environment.apiBaseUrl}api/family/warranty-records`,
    params: {
      page: this.page(),
      pageSize: this.pageSize(),
    },
    withCredentials: true,
}));

  readonly warrantyRecord = httpResource<WarrantyRecordResponse>(() =>
    this.warrantyId()
      ? {
          url: `${environment.apiBaseUrl}api/family/warranty-records/${this.warrantyId()}`,
          withCredentials: true,
        }
      : undefined
  );

  create(request: CreateWarrantyRecordRequest): Observable<WarrantyRecordResponse> {
    const formData = new FormData();
    formData.append('warrantyBrandId', request.warrantyBrandId);
    formData.append('model', request.model);
    formData.append('purchaseDate', request.purchaseDate);

    if (request.receipt) {
      formData.append('receipt', request.receipt);
    }
    if (request.frontImage) {
      formData.append('frontImage', request.frontImage);
    }
    if (request.backImage) {
      formData.append('backImage', request.backImage);
    }

    return this.#http.post<WarrantyRecordResponse>(
      `${environment.apiBaseUrl}api/family/warranty-records`,
      formData,
      { withCredentials: true }
    );
  }
}
