import { HttpClient, httpResource } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateWarrantyRecordRequest } from '../models/create-warranty-record.request';
import { WarrantyRecordResponse } from '../models/warranty-record.response';
import { AuthService } from '../../../../core/auth/auth.service';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';
import { inlineErrors } from '../../../../core/http/http-context';
import { PaginationResponse } from '../../../../shared/pagination/pagination-response.model';

export interface WarrantyRecordListResponse {
  id: string;
  warrantyBrandName: string;
  model: string;
  /** ISO 8601 date string as sent by the API. */
  purchaseDate: string;
  duration: number;
  /** ISO 8601 date string as sent by the API. */
  endDate: string;
  receiptExists: boolean;
  frontImageExists: boolean;
  backImageExists: boolean;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class WarrantyRecordsService {
  readonly #http = inject(HttpClient);
  readonly #authService = inject(AuthService);

  readonly page = signal(1);
  readonly pageSize = signal(10);

  /**
   * Keyed on the signed-in user: the resource goes idle and drops its value on logout and
   * refetches for the next user, so one user's warranties never show up for another.
   */
  readonly warrantyRecords = httpResource<PaginationResponse<WarrantyRecordListResponse>>(() =>
    this.#authService.userId()
      ? {
          url: apiUrl(Endpoints.warrantyRecords),
          params: { page: this.page(), pageSize: this.pageSize() },
        }
      : undefined
  );

  constructor() {
    // A new session starts on the first page.
    effect(() => {
      if (!this.#authService.userId()) {
        this.page.set(1);
      }
    });
  }

  create(request: CreateWarrantyRecordRequest): Observable<WarrantyRecordResponse> {
    const formData = new FormData();
    formData.append('warrantyBrandId', request.warrantyBrandId);
    formData.append('model', request.model);
    formData.append('purchaseDate', request.purchaseDate);
    formData.append('duration', request.duration.toString());

    if (request.receipt) {
      formData.append('receipt', request.receipt);
    }
    if (request.frontImage) {
      formData.append('frontImage', request.frontImage);
    }
    if (request.backImage) {
      formData.append('backImage', request.backImage);
    }

    return this.#http.post<WarrantyRecordResponse>(apiUrl(Endpoints.warrantyRecords), formData, {
      context: inlineErrors(),
    });
  }

  delete(id: string): Observable<void> {
    return this.#http.delete<void>(apiUrl(Endpoints.warrantyRecords, id));
  }
}
