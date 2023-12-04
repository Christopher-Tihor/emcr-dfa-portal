import {
  Component,
  OnInit,
  AfterViewChecked,
  ChangeDetectorRef
} from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as globalConst from '../../core/services/globalConstants';
import { MatStepper } from '@angular/material/stepper';
import { Subscription } from 'rxjs';
import { FormCreationService } from '../../core/services/formCreation.service';
import { DFAPrescreeningDataService } from './dfa-prescreening-data.service';
import { ApplicantOption } from 'src/app/core/api/models';
import { MatDialog } from '@angular/material/dialog';
import { DFAConfirmPrescreeningDialogComponent } from 'src/app/core/components/dialog-components/dfa-confirm-prescreening-dialog/dfa-confirm-prescreening-dialog.component';
import { LoginService } from 'src/app/core/services/login.service';

@Component({
  selector: 'app-dfa-prescreening',
  templateUrl: './dfa-prescreening.component.html',
  styleUrls: ['./dfa-prescreening.component.scss']
})
export class DFAPrescreeningComponent
  implements OnInit, AfterViewChecked
{
  dfaPrescreeningFolderPath = 'dfa-prescreening-forms';
  path: string;
  form$: Subscription;
  form: UntypedFormGroup;
  currentFlow: string;
  type = 'dfa-prescreening';
  parentPageName = 'dfa-dashboard';
  showLoader = false;
  ApplicantOptions = ApplicantOption;
  dfaPrescreeningForm: UntypedFormGroup;
  dfaPrescreeningForm$: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formCreationService: FormCreationService,
    private cd: ChangeDetectorRef,
    public dfaPrescreeningDataService: DFAPrescreeningDataService,
    public dialog: MatDialog,
    private loginService: LoginService
  ) {
    const navigation = this.router.getCurrentNavigation();
  }

  clearAnswers() {
    this.dfaPrescreeningDataService.clearPreScreeningAnswers.emit();
  }

  ngOnInit(): void {

    this.loadStepForm();
    this.currentFlow = this.route.snapshot.data.flow ? this.route.snapshot.data.flow : 'verified-registration';

    // clear old data
    this.dfaPrescreeningDataService.dfaPrescreening = null;
    this.formCreationService.clearDfaPrescreeningData();

    this.dfaPrescreeningForm$ = this.formCreationService
      .getDfaPrescreeningForm()
      .subscribe((prescreening) => {
        this.dfaPrescreeningForm = prescreening;
      });
  }

  ngAfterViewChecked(): void {
    this.cd.detectChanges();
  }

  /**
   * Custom back stepper function
   *
   * @param stepper stepper instance
   * @param lastStep stepIndex
   */
  goBack(stepper: MatStepper, lastStep): void {
    this.returnToDashboard();
  }

  /**
   * Custom next stepper function
   *
   * @param stepper stepper instance
   * @param isLast stepperIndex
   * @param component current component name
   */
  goForward(component: string): void {
    if (this.form.valid) {
      this.setFormData();
      this.submitFile();
    }
    this.form$.unsubscribe();
    this.form.markAllAsTouched();
  }

  /**
   * Sets the form data to the DTO services
   *
   * @param component Name of the component
   */
  setFormData(): void {
    this.dfaPrescreeningDataService.dfaPrescreening = {
      addressLine1: this.form.get('addressLine1').value,
      addressLine2: this.form.get('addressLine2').value,
      city: this.form.get('city').value,
      eventId: this.form.get('eventId').value,
      postalCode: this.form.get('postalCode').value,
      stateProvince: this.form.get('stateProvince').value,
      applicantOption: this.form.get('applicantOption').value,
      insuranceOption: this.form.get('insuranceOption').value,
      damageFromDate: this.form.get('damageFromDate').value,
      damageCausedByDisaster: this.form.get('damageCausedByDisaster').value == 'true' ? true : (this.form.get('damagedCausedByDisaster').value == 'false' ? false : null),
      isPrimaryAndDamagedAddressSame: this.form.get('isPrimaryAndDamagedAddressSame').value == 'true' ? true : (this.form.get('isPrimaryAndDamagedAddressSame').value == 'false' ? false : null),
      isDamagedAddressVerified: this.form.get('isDamagedAddressVerified').value == 'true' ? true : (this.form.get('isDamagedAddressVerified').value == 'false' ? false : null),
      lossesExceed1000: this.form.get('lossesExceed1000').value == 'true' ? true : (this.form.get('lossesExceed1000').value == 'false' ? false : null),
      profileId: this.form.get('profileId').value,
    }
  }

  /**
   * Loads appropriate forms based on the current step
   *
   * @param index Step index
   */
  loadStepForm(): void {
    this.form$ = this.formCreationService
      .getDfaPrescreeningForm()
      .subscribe((dfaPrescreening) => {
        this.form = dfaPrescreening;
      });
  }

  returnToDashboard() {
    this.router.navigate(['/dfa-dashboard']);
  }

  submitFile(): void {
    this.dialog
      .open(DFAConfirmPrescreeningDialogComponent, {
        data: {
          content: globalConst.confirmPrescreeningBody
        },
        width: '700px',
        disableClose: true
      })
      .afterClosed()
      .subscribe((result) => {
        if (result === 'confirm' && this.loginService.isLoggedIn()) {
          const navigationPath = '/dfa-application-start';
          this.router.navigate([navigationPath]);
        } else this.returnToDashboard();
      });
  }
}
