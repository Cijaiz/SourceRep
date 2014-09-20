<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="NotificationHub.Cloud" generation="1" functional="0" release="0" Id="c3e0ab67-1d6b-47bf-871c-985ca73cdd01" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="NotificationHub.CloudGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="NotificationHub:HttpsIn" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/LB:NotificationHub:HttpsIn" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|NotificationHub:HTTPS.Cert" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapCertificate|NotificationHub:HTTPS.Cert" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:AcsNamespace" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:AcsNamespace" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:AcsRealm" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:AcsRealm" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:AcsTokenKey" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:AcsTokenKey" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:C2CDataEntities" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:C2CDataEntities" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:DiagnosticLogLevel" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:DiagnosticLogLevel" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="NotificationHub:NotificationHubEntities" defaultValue="">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHub:NotificationHubEntities" />
          </maps>
        </aCS>
        <aCS name="NotificationHubInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/MapNotificationHubInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:NotificationHub:HttpsIn">
          <toPorts>
            <inPortMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/HttpsIn" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCertificate|NotificationHub:HTTPS.Cert" kind="Identity">
          <certificate>
            <certificateMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/HTTPS.Cert" />
          </certificate>
        </map>
        <map name="MapNotificationHub:AcsNamespace" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/AcsNamespace" />
          </setting>
        </map>
        <map name="MapNotificationHub:AcsRealm" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/AcsRealm" />
          </setting>
        </map>
        <map name="MapNotificationHub:AcsTokenKey" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/AcsTokenKey" />
          </setting>
        </map>
        <map name="MapNotificationHub:C2CDataEntities" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/C2CDataEntities" />
          </setting>
        </map>
        <map name="MapNotificationHub:DiagnosticLogLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/DiagnosticLogLevel" />
          </setting>
        </map>
        <map name="MapNotificationHub:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapNotificationHub:NotificationHubEntities" kind="Identity">
          <setting>
            <aCSMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/NotificationHubEntities" />
          </setting>
        </map>
        <map name="MapNotificationHubInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHubInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="NotificationHub" generation="1" functional="0" release="0" software="E:\Projects\OneCampus\NotificationHub\NotificationHub.Cloud\csx\Debug\roles\NotificationHub" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="HttpsIn" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/HTTPS.Cert" />
                </certificate>
              </inPort>
            </componentports>
            <settings>
              <aCS name="AcsNamespace" defaultValue="" />
              <aCS name="AcsRealm" defaultValue="" />
              <aCS name="AcsTokenKey" defaultValue="" />
              <aCS name="C2CDataEntities" defaultValue="" />
              <aCS name="DiagnosticLogLevel" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="NotificationHubEntities" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;NotificationHub&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;NotificationHub&quot;&gt;&lt;e name=&quot;HttpsIn&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0HTTPS.Cert" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub/HTTPS.Cert" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="HTTPS.Cert" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHubInstances" />
            <sCSPolicyUpdateDomainMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHubUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHubFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="NotificationHubUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="NotificationHubFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="NotificationHubInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="876e5bc4-f382-4453-963f-5b88898f0a7f" ref="Microsoft.RedDog.Contract\ServiceContract\NotificationHub.CloudContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="efe4fd95-a83e-484e-8b58-441cce73ce1e" ref="Microsoft.RedDog.Contract\Interface\NotificationHub:HttpsIn@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/NotificationHub.Cloud/NotificationHub.CloudGroup/NotificationHub:HttpsIn" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>