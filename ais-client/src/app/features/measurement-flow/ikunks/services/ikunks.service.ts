import { httpResource } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';
import { IkunksPagedResponse } from '../models/ikunks-paged.response';



@Injectable()
export class IkunksService {
  readonly ikunks = httpResource<IkunksPagedResponse>(() => ({
    url: apiUrl(Endpoints.ikunks),
  }));
}
