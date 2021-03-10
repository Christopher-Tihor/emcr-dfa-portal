import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopNavMenuComponent } from './components/top-nav-menu/top-nav-menu.component';
import { RouterModule } from '@angular/router';
import { ToggleSideNavComponent } from './components/toggle-side-nav/toggle-side-nav.component';
import { MaterialModule } from '../material.module';
import { DataTableComponent } from './components/data-table/data-table.component';
import { SearchFilterComponent } from './components/search-filter/search-filter.component';
import { FormsModule } from '@angular/forms';
import { DialogComponent } from './components/dialog/dialog.component';
import { AlertComponent } from './components/alert/alert.component';
import { AppLoaderComponent } from './components/app-loader/app-loader.component';
import { EnumToArrayPipe } from './pipes/EnumToArray.pipe';

@NgModule({
  declarations: [
    TopNavMenuComponent,
    ToggleSideNavComponent,
    DataTableComponent,
    SearchFilterComponent,
    DialogComponent,
    AlertComponent,
    AppLoaderComponent,
   // EnumToArrayPipe
  ],
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    FormsModule
  ],
  exports: [
    TopNavMenuComponent,
    ToggleSideNavComponent,
    DataTableComponent,
    SearchFilterComponent,
    DialogComponent,
    AlertComponent,
    AppLoaderComponent,
   // EnumToArrayPipe
  ]
})
export class SharedModule { }