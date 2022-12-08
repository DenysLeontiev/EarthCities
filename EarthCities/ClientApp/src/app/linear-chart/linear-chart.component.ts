import { Component, OnInit } from '@angular/core';
import { DataChartService } from '../data-chart.service';

@Component({
  selector: 'app-linear-chart',
  templateUrl: './linear-chart.component.html',
  styleUrls: ['./linear-chart.component.css']
})
export class LinearChartComponent implements OnInit {

  constructor(/*private dataChartService: DataChartService*/) { }

  ngOnInit() {
    //this.dataChartService.displayDataChart().subscribe((response) => {
    //  console.log(response);
    //})
  }

}
