import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CreateWarrantyRecordRequest } from '../models/create-warranty-record.request';
import { WarrantyRecordResponse } from '../models/warranty-record.response';

@Injectable({ providedIn: 'root' })
export class WarrantyRecordsService {
  readonly #http = inject(HttpClient);

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
