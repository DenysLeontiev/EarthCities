import { TestBed } from '@angular/core/testing';

import { DataChartService } from './data-chart.service';

describe('DataChartService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DataChartService = TestBed.get(DataChartService);
    expect(service).toBeTruthy();
  });
});
