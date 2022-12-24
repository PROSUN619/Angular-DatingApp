import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/Message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
   // to get rid of ExpressionChangedAfterItHasBeenCheckedError error in development environment
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  @Input() messages: Message[] = [];
  messageContent = '';


  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    //this.loadMessages();
  }

  sendMessage() {
    if (this.username && !this.messageForm.invalid) {
      //console.log(this.messageForm.invalid);
      //console.log(this.messageContent);
      this.messageService.sendMessage(this.username, this.messageContent).then(() => {
        this.messageForm.reset();
      });
    }
  }


}
