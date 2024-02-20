
// team.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamService } from 'src/app/services/team.service';

@Component({
  selector: 'app-add-team',
  templateUrl: './add-team.component.html',
  styleUrls: ['./add-team.component.css']  // Add your styles if needed
})
export class AddTeamComponent implements OnInit {
  teamForm: FormGroup;
  errorMessage: string;
  photoImage: string;
  isEditMode: boolean = false;

  constructor(private fb: FormBuilder, private router: Router, private teamService: TeamService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.initializeForm();
    this.isEditMode = this.route.snapshot.url[1]?.path === 'edit';

    if (this.isEditMode) {
      this.loadTeamData();
    }
  }

  loadTeamData(): void {
    const teamId = this.route.snapshot.paramMap.get('id');
    this.teamService.getTeamById(teamId).subscribe(
      (team) => {
        console.log("team in edit is ",team);
        // Prefill the form with team data
        this.teamForm.patchValue({
          TeamName: team.TeamName,
          TeamImageURL: this.photoImage,
          TeamDescription: team.TeamDescription,
          TeamId: team.TeamId
        });

        // If Players are available, you may handle them accordingly
        // if (team.Players) {
        //   // Handle player data if needed
        // }
      },
      (error) => {
        console.error('Error fetching team data', error);
      }
    );
  }

  initializeForm(): void {
    this.teamForm = this.fb.group({
      TeamName: ['', Validators.required],
      TeamImageURL: ['', Validators.required],
      TeamDescription: ['', Validators.required],
      TeamId: ['']
    });
  }

  goBack(): void {
    // Add logic to navigate back if needed
    this.router.navigate(['/admin/team/dashboard']);
  }

  onSubmit(): void {
    if (this.teamForm.valid) {
      if (this.isEditMode) {
        // Handle updating an existing team
        const team = {
          TeamName: this.teamForm.get('TeamName').value,
          TeamImageURL: this.photoImage,
          TeamDescription: this.teamForm.get('TeamDescription').value,
          TeamId: this.teamForm.get('TeamId').value
        };

        
       console.log("team",team)
        this.teamService.updateTeam(team).subscribe(
          (response) => {
            console.log('Team updated successfully', response);
            this.router.navigate(['/admin/team/dashboard']);
          },
          (error) => {
            console.error('Error updating team', error);
          }
        );
      } else {
        // Handle creating a new team
        const team = {
          TeamName: this.teamForm.get('TeamName').value,
          TeamImageURL: this.photoImage,
          TeamDescription: this.teamForm.get('TeamDescription').value
        };

        this.teamService.addTeam(team).subscribe(
          (response) => {
            console.log('Team added successfully', response);
            this.teamForm.reset();
            this.router.navigate(['/admin/team/dashboard']);
          },
          (error) => {
            console.error('Error adding team', error);
          }
        );
      }
    } else {
      this.errorMessage = 'Please fill out all required fields.';
    }
  }

  handleFileChange(event: any): void {
    const file = event.target.files[0];

    if (file) {
      this.convertFileToBase64(file).then(
        (base64String) => {
          this.photoImage=base64String
        },
        (error) => {
          console.error('Error converting file to base64:', error);
          // Handle error appropriately
        }
      );
    }
  }

  convertFileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        resolve(reader.result as string);
      };

      reader.onerror = (error) => {
        reject(error);
      };

      reader.readAsDataURL(file);
    });
  }


}
