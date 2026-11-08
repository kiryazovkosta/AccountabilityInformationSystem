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
