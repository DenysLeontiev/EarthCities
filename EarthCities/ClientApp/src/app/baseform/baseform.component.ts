import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  template: ''
})
export abstract class BaseformComponent {

  constructor() { }

  form: FormGroup;

  getControl(name: string ) {
    return this.form.get(name);
  }

  hasError(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched) && e.invalid;
  }

  isChanged(name: string) {
    var e = this.getControl(name);
    return e && (e.touched || e.dirty);
  }

  isValid(name: string) {
    var e = this.getControl(name);
    return e && e.valid;
  }
}
