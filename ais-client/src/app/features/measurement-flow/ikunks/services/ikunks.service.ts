import { httpResource } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { IkunksPagedResponse } from '../models/ikunks-paged.response';



@Injectable()
export class IkunksService {
  readonly ikunks = httpResource<IkunksPagedResponse>(() => ({
    url: `${environment.apiBaseUrl}api/flow/ikunks`,
    withCredentials: true,
  }));
}
