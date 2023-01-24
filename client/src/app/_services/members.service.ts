import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.dev';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl=environment.apiUrl;
  //we create a member property in service (as we have in list-component) in order not to hit API every sing time when component is destroyed.
  members:Member[]=[];

  constructor(private http: HttpClient) { }

  getMembers(){
    if(this.members.length>0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl+'users').pipe(
      map(members=>{
        this.members=members;
        return members;
      })
    )
  }

  getMember(username:string){
    const member=this.members.find(x=>x.userName==username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl+'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        this.members[index]={...this.members[index],...member}
      })
    )
  }

 
}
