import { Injectable } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { first } from 'rxjs/operators';
import { DfaApplicationMain, FullTimeOccupant, SecondaryApplicant, OtherContact, DamagedRoom, FileUpload, CleanUpLogItem } from 'src/app/core/model/dfa-application-main.model';
import { DFAApplicationMainDataService } from './dfa-application-main-data.service';
import { FormCreationService } from '../../core/services/formCreation.service';

@Injectable({ providedIn: 'root' })
export class DFAApplicationMainMappingService {
  constructor(
    private formCreationService: FormCreationService,
    private dfaApplicationMainDataService: DFAApplicationMainDataService,
  ) {}

  mapDFAApplicationMain(dfaApplicationMain: DfaApplicationMain): void {
    this.dfaApplicationMainDataService.setDFAApplicationMain(dfaApplicationMain);
    this.setExistingDFAApplicationMain(dfaApplicationMain);
  }

  setExistingDFAApplicationMain(dfaApplicationMain: DfaApplicationMain): void {
    this.setPropertyDamageDetails(dfaApplicationMain);
  }

  private setPropertyDamageDetails(dfaApplicationMain: DfaApplicationMain): void {
    let formGroup: UntypedFormGroup;
    this.formCreationService
      .getPropertyDamageForm()
      .pipe(first())
      .subscribe((propertyDamage) => {
        propertyDamage.setValue({
          ...dfaApplicationMain.propertyDamage,
        });
        formGroup = propertyDamage;
      });
    this.dfaApplicationMainDataService.propertyDamage = dfaApplicationMain.propertyDamage;
  }

}
